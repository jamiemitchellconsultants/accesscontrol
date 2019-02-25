using System;
using AccessControl.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AccessControl.Models
{
    public partial class AccessControlContext : DbContext
    {
        public AccessControlContext()
        {
        }

        public AccessControlContext(DbContextOptions<AccessControlContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Action> Action { get; set; }
        public virtual DbSet<Applicationarea> Applicationarea { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<Grouprole> Grouprole { get; set; }
        public virtual DbSet<Jwtissuer> Jwtissuer { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Resource> Resource { get; set; }
        public virtual DbSet<Resourceaction> Resourceaction { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Usergroup> Usergroup { get; set; }
        public virtual DbQuery<UserPermissionCheck> PermissionCheck { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=localhost;user id=root;persistsecurityinfo=True;database=AccessControl;allowuservariables=True;password=One0too4.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Query<UserPermissionCheck>(entity =>
            {
                entity.ToView("userpermissions", "accesscontrol");

                entity.Property(e => e.LocalName).HasColumnName("LocalName").HasMaxLength(255);
                entity.Property(e => e.Deny).HasColumnName("Deny").HasConversion<int>();
                entity.Property(e => e.ActionName).HasColumnName("ActionName").HasMaxLength(255);
                entity.Property(e => e.ResourceName).HasColumnName("ResourceName").HasMaxLength(255);

                entity.Property(e => e.ActionId)
                    .HasColumnName("ActionId")
                    .HasColumnType("char(36)");

                entity.Property(e => e.ResourceId)
                    .HasColumnName("ResourceId")
                    .HasColumnType("char(36)");

                entity.Property(e => e.PermissionId)
                    .HasColumnName("PermissionId")
                    .HasColumnType("char(36)");

                entity.Property(e => e.ResourceActionId)
                    .HasColumnName("ResourceActionId")
                    .HasColumnType("char(36)");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("SubjectId")
                    .HasColumnType("char(36)");


            });

            modelBuilder.Entity<Action>(entity =>
            {
                entity.ToTable("action", "accesscontrol");

                entity.HasIndex(e => e.ActionName)
                    .HasName("ActionName")
                    .IsUnique();

                entity.Property(e => e.ActionId)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActionName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Applicationarea>(entity =>
            {
                entity.ToTable("applicationarea", "accesscontrol");

                entity.HasIndex(e => e.ApplicationAreaName)
                    .HasName("ApplicationAreaName")
                    .IsUnique();

                entity.Property(e => e.ApplicationAreaId)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.ApplicationAreaName)
                    .IsRequired()
                    .HasColumnType("char(255)");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("group", "accesscontrol");

                entity.HasIndex(e => e.GroupName)
                    .HasName("GroupName")
                    .IsUnique();

                entity.Property(e => e.GroupId)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Grouprole>(entity =>
            {
                entity.ToTable("grouprole", "accesscontrol");

                entity.HasIndex(e => e.GroupId)
                    .HasName("GroupId");

                entity.HasIndex(e => e.RoleId)
                    .HasName("RoleId");

                entity.HasIndex(e => new { e.RoleId, e.GroupId })
                    .HasName("RoleGroup")
                    .IsUnique();

                entity.Property(e => e.GroupRoleId)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.GroupId)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Grouprole)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_grouprole_group");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Grouprole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_grouprole_role");
            });

            modelBuilder.Entity<Jwtissuer>(entity =>
            {
                entity.ToTable("jwtissuer", "accesscontrol");

                entity.Property(e => e.JwtissuerId)
                    .HasColumnName("JWTissuerId")
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.IssuerName)
                    .IsRequired()
                    .HasColumnType("char(255)");

                entity.Property(e => e.SubjectClaimName)
                    .IsRequired()
                    .HasColumnType("char(255)");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permission", "accesscontrol");

                entity.HasIndex(e => e.ResourceActionId)
                    .HasName("permissionresourceaction");

                entity.HasIndex(e => e.RoleId)
                    .HasName("permissionrole");

                entity.HasIndex(e => new { e.RoleId, e.ResourceActionId })
                    .HasName("uniquepermission")
                    .IsUnique();

                entity.Property(e => e.PermissionId)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Deny).HasColumnType("tinyint(1)");

                entity.Property(e => e.ResourceActionId)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.HasOne(d => d.ResourceAction)
                    .WithMany(p => p.Permission)
                    .HasForeignKey(d => d.ResourceActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_permission_resourceaction");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Permission)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_permission_role");
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.ToTable("resource", "accesscontrol");

                entity.HasIndex(e => e.ApplicationAreaId)
                    .HasName("resourceapplicationarea");

                entity.HasIndex(e => e.ResourceName)
                    .HasName("resourcename")
                    .IsUnique();

                entity.Property(e => e.ResourceId)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.ApplicationAreaId)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.Property(e => e.ResourceName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.ApplicationArea)
                    .WithMany(p => p.Resource)
                    .HasForeignKey(d => d.ApplicationAreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Resoruce_Apparea");
            });

            modelBuilder.Entity<Resourceaction>(entity =>
            {
                entity.ToTable("resourceaction", "accesscontrol");

                entity.HasIndex(e => e.ActionId)
                    .HasName("resourceactionaction");

                entity.HasIndex(e => e.ResourceId)
                    .HasName("resourceactionresource");

                entity.HasIndex(e => new { e.ActionId, e.ResourceId })
                    .HasName("uniqueresourceaction")
                    .IsUnique();

                entity.Property(e => e.ResourceActionId)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActionId)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.Property(e => e.ResourceId)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.Resourceaction)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_resourceaction_action");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.Resourceaction)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_resourceaction_resource");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role", "accesscontrol");

                entity.HasIndex(e => e.RoleName)
                    .HasName("rolename")
                    .IsUnique();

                entity.Property(e => e.RoleId)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasColumnName("Role Name")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", "accesscontrol");

                entity.HasIndex(e => e.LocalName)
                    .HasName("localname")
                    .IsUnique();

                entity.HasIndex(e => e.SubjectId)
                    .HasName("usersubjectid")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.LocalName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SubjectId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usergroup>(entity =>
            {
                entity.ToTable("usergroup", "accesscontrol");

                entity.HasIndex(e => e.GroupId)
                    .HasName("Index 2");

                entity.HasIndex(e => e.UserId)
                    .HasName("Index 3");

                entity.HasIndex(e => new { e.UserId, e.GroupId })
                    .HasName("Index 4")
                    .IsUnique();

                entity.Property(e => e.UserGroupId)
                    .HasColumnType("char(36)")
                    .ValueGeneratedNever();

                entity.Property(e => e.GroupId)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Usergroup)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_usergroup_group");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Usergroup)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_usergroup_user");
            });
        }
    }
}
